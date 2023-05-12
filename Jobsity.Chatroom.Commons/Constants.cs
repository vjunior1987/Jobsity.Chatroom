using System;

namespace Jobsity.Chatroom.Commons
{
    public static class Constants
    {
        public const string CHATBOT_HELP_MESSAGE = "Use the command \"/stock=[stock_code]\", where [stock_code] is the identifier of the stock you wish to inquiry";
        public const string CHATBOT_STOCK_NOT_FOUND_MESSAGE = "Stock queue not found. Try again with another code";
        public const string CHATBOT_COMMAND_NOT_RECOGNIZED_MESSAGE = "command not recognized. Use /help for more info.";
        public const string CHATBOT_UNKNOWN_ERROR_MESSAGE = "there was an error when executing the command";
        public const string CHATBOT_STOCK_QUOTE_PER_SHARE_MESSAGE = "“{0} quote is ${1} per share”.";
    }
}
