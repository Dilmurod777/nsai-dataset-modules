using Instances;
using UnityEngine;

public class ContextManager: MonoBehaviour
{
    public Task CurrentTask;
    public Subtask CurrentSubtask;
    public Instruction CurrentInstruction;
    public Query CurrentQuery;
    
    public void SetCurrentTask(Task task)
    {
        CurrentTask = task;
    }

    public void SetCurrentSubtask(Subtask subtask)
    {
        CurrentSubtask = subtask;
    }

    public void SetCurrentInstruction(Instruction instruction)
    {
        CurrentInstruction = instruction;
    }

    public void SetCurrentQuery(Query query)
    {
        CurrentQuery = query;
    }
}