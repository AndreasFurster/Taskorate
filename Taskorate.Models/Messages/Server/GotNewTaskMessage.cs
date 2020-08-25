namespace Taskorate.Models.Messages
{
  public class GotNewTaskMessage : ServerMessage
  {
    public const string MethodName = nameof(GotNewTaskMessage);
    public QuickTask QuickTask { get; set; }

    public GotNewTaskMessage()
    {
        
    }
    public GotNewTaskMessage(QuickTask task)
    {
      QuickTask = task;
    }
  }
}