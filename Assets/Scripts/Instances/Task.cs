using System.Collections.Generic;

namespace Instances
{
    public class Task
    {
        public string TaskId;
        public string Title;
        public List<Figure> Figures;
        public List<Subtask> Subtasks;
        public bool isCompleted;

        public Task(string taskId, string title, List<Figure> figures, List<Subtask> subtasks)
        {
            TaskId = taskId;
            Title = title;
            Figures = figures;
            Subtasks = subtasks;
            isCompleted = false;
        }

        public void SetIsCompleted(bool state)
        {
            isCompleted = state;
        }
    }
}