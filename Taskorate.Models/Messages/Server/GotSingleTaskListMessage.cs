namespace Taskorate.Models.Messages
{
  public class GotSingleTaskListMessage : ServerMessage
  {
    public const string MethodName = nameof(GotSingleTaskListMessage);
    public QuickTaskList TaskList { get; set; }

    public GotSingleTaskListMessage()
    {
        
    }

    public GotSingleTaskListMessage(QuickTaskList taskList)
    {
      TaskList = taskList;
    }
  }
}