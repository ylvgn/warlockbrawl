using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerManager : MonoBehaviour
{
    private int ids; // 自增timerId, 0表示无效id
    private Dictionary<int, TimerData> timerIdToHandlerDict;
    private List<int> waitingRemoveList;
    private List<TimerData> waitingAddList;
    public static TimerManager Instance => _instance;
    static TimerManager _instance = null;
    public sealed class TimerData
    {
        public readonly int timerId;
        public readonly bool isLoop;
        private Action func;
        private float delayTime;
        private float intervalCallBackTime; // LoopTimer的间隔执行时间
        private float startTime;
        private bool hasResetLoopData;
        public TimerData(int timerId_, float delayTime_, Action func_, float intervalCallBackTime_ = 0)
        {
            if (func_ == null) {
                Debug.LogError("TimerData传入function为空");
            }

            timerId = timerId_;
            delayTime = delayTime_;
            func = func_;
            intervalCallBackTime = intervalCallBackTime_;
            startTime = Time.realtimeSinceStartup;

            if (intervalCallBackTime > 0){
                isLoop = true;
                hasResetLoopData = false;
            } else hasResetLoopData = true;
        }

        public bool CanAction()
        {
            if (IsStop()) return false;

            float endTime = startTime + delayTime;
            if(Time.realtimeSinceStartup >= endTime) {
                return true;
            }
            return false;
        }

         // 如果是loop, 在第一次执行handler之后：delayTime要更新成intervalCallBackTime
        public void ResetLoopTimerData() {
            if (hasResetLoopData) return;
            delayTime = intervalCallBackTime;
            hasResetLoopData = true;
        }

        // 每次handle之后都更新startTime
        public void Handle() {
            if (IsStop()) {
                return;
            }
            startTime = Time.realtimeSinceStartup;
            func();
        }

        public void Stop() {
            func = null;
        }

        public bool IsStop() {
            return func == null;
        }
    }

    void Awake()
    {
        if (_instance != null) {
            GameObject.Destroy(_instance); 
            Debug.LogError("重复实例化 TimerManager.instance");
        }
        _instance = this;
        ids = 0;
        waitingRemoveList = new List<int>();
        waitingAddList = new List<TimerData>();
        timerIdToHandlerDict = new Dictionary<int, TimerData>();
    }

    void Update()
    {
        if (waitingAddList.Count > 0) {
            foreach(var item in waitingAddList) {
                timerIdToHandlerDict.Add(item.timerId, item);
            }
            waitingAddList.Clear();
        }

        if (waitingRemoveList.Count > 0) {
            foreach(int timerId in waitingRemoveList) {
                timerIdToHandlerDict.Remove(timerId);
            }
            Debug.Log(string.Format("[TimerManager]: 清理了{0}个timer， 还剩余{1}个timer", waitingRemoveList.Count, timerIdToHandlerDict.Count));
            waitingRemoveList.Clear();
        }

        if (timerIdToHandlerDict.Count > 0) {
            foreach (var item in timerIdToHandlerDict) {
                int timerId = item.Key;
                var handler = item.Value;
                if (!handler.CanAction()) continue;
                handler.Handle();
                if (!handler.isLoop) {
                    RemoveTimer(timerId);
                } else {
                    handler.ResetLoopTimerData();
                }
            }
        }
    }


    // 延迟执行1次
    public int Add(float delaySecond, Action function)
    {
        if (function == null) {
            Debug.Log("TimerManager.Add 传入function为空");
            return 0;
        }
        if (delaySecond <= 0) {
            function();
            return 0;
        }
        ids ++;
        if (ids == Int32.MaxValue || timerIdToHandlerDict.ContainsKey(ids)) {
            for (int k = 1; k <= timerIdToHandlerDict.Count + 1; k ++) {
                if (!timerIdToHandlerDict.ContainsKey(k)) {
                    ids = k;
                    break;
                }
            }
            Debug.Log("TimerManager.Add ids重置为:" + ids);
        }
        
        waitingAddList.Add(new TimerData(ids, delaySecond, function));
        return ids;  // 返回timerId, 可外部控制中断
    }

    // 延迟+循环执行
    public int AddLoop(float delaySecond, float intervalSecond, Action function)
    {
        if (function == null) {
            Debug.Log("TimerManager.AddLoop 传入function为空");
            return 0;
        }
        if (delaySecond <= 0) {
            function();
            return 0;
        }
        if (intervalSecond < 0) {
            Debug.LogError("TimerManager.AddLoop 传入intervalSecond必须为非负数! intervalSecond= " + intervalSecond);
            return 0;
        }
        ids ++;
        if (ids == Int32.MaxValue || timerIdToHandlerDict.ContainsKey(ids)) {
            for (int k = 1; k <= timerIdToHandlerDict.Count + 1; k ++) {
                if (!timerIdToHandlerDict.ContainsKey(k)) {
                    ids = k;
                    break;
                }
            }
            Debug.Log("TimerManager.AddLoop ids重置为：" + ids);
        }
        
        waitingAddList.Add(new TimerData(ids, delaySecond, function, intervalSecond));
        return ids;
    }

    public void RemoveTimer(int timerId)
    {
        if (timerId == 0) return;
        TimerData handler;
        if (timerIdToHandlerDict.TryGetValue(timerId, out handler)) {
            if (!handler.IsStop()) {
                waitingRemoveList.Add(timerId);
            }
            handler.Stop();
        }
    }
}
