namespace Taskorate.Models.Messages
{
  public class GetSingleTaskListMessage
  {
    public const string MethodName = nameof(GetSingleTaskListMessage);
    public string Id { get; set; }

    public GetSingleTaskListMessage()
    {
        
    }
    public GetSingleTaskListMessage(string id)
    {
      Id = id;
    }
  }
}