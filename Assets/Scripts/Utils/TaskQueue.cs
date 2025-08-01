using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Utils
{
    public class TaskQueue
    {
        private readonly Queue<Func<Task>> _taskQueue = new();
        private bool _isProcessing = false;

        public void Enqueue(Func<Task> taskFunc)
        {
            _taskQueue.Enqueue(taskFunc);
            if (!_isProcessing)
            {
                _isProcessing = true;
                _ = ProcessQueue();
            }
        }

        public void ClearQueue()
        {
            _taskQueue.Clear();
        }

        private async Task ProcessQueue()
        {
            while (_taskQueue.Count > 0)
            {
                var taskFunc = _taskQueue.Dequeue();
                await taskFunc();
            }
            _isProcessing = false;
        }
    }
}