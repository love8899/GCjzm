namespace Wfm.Services.Messages
{
    public partial interface ITextMessageSender
    {
        int SendTextMessage(string message, string numbers);
    }
}
