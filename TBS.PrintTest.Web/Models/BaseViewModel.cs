using Foundation.Core.Enums;
using Foundation.Core.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace TBS.PrintTest.Web.Models
{
    public class BaseViewModel
    {
        private List<Message> messages = new List<Message>();


        public List<Message> Errors { get { return Messages.Where(m => m.Type == MessageType.Error).ToList(); } }
        public List<Message> Infos { get { return Messages.Where(m => m.Type == MessageType.Information).ToList(); } }
        public List<Message> Messages { get { return messages; } }
        public Message Success { get { return Messages.FirstOrDefault(m => m.Type == MessageType.Success); } }
        public List<Message> Warnings { get { return Messages.Where(m => m.Type == MessageType.Warning).ToList(); } }
        public OperationType? OperationType { get; set; }

        public BaseViewModel() { }

        public void AddErrorMessage(Result result, string text, string id = null)
        {
            AddErrorMessage(text, id);
        }
        public void AddErrorMessage(string text, string id = null)
        {
            var message = new Message(MessageType.Error, id, string.Empty, text);
            Messages.Add(message);
        }
        public void AddInformationMessage(string text, string id = null)
        {
            var message = new Message(MessageType.Information, id, text);
            Messages.Add(message);
        }
        public void AddWarningMessage(string text, string id = null)
        {
            var message = new Message(MessageType.Warning, id, text);
            Messages.Add(message);
        }

        public void ClearMessages(MessageType? type = null)
        {
            if (type.HasValue)
                Messages.RemoveAll(e => e.Type == (MessageType)type);
            else
                Messages.Clear();
        }

        public void ProcessModelStateErrorMessages(ModelStateDictionary state)
        {
            foreach (string k in state.Keys)
                if (state[k].Errors.Count > 0)
                    foreach (var e in state[k].Errors)
                        AddErrorMessage(e.ErrorMessage, k);
        }

        /// <summary>
        /// Processes the status and messages of the Result object and returns the integer value of the Result object - if the operation was successful.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Value of the Result object - if the operation was successful.</returns>
        public int ProcessResult(Result result)
        {
            ProcessResultMessages(result.Messages, result.Status);
            if (result.Status == OperationStatus.Success)
                return result.Value;
            else
                return default;
        }

        /// <summary>
        /// Processes the status and messages of the Result object and returns the typed value of the Result object - if the operation was successful.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Typed value of the Result object - if the operation was successful.</returns>
        public T ProcessResult<T>(Result<T> result)
        {
            ProcessResultMessages(result.Messages, result.Status);
            if (result.Status == OperationStatus.Success)
                return result.Value;
            else
                return default;
        }

        /// <summary>
        /// Processes the messages of a Result object and adds them to the ViewModel. For certain operation types, provide a default success message.
        /// </summary>
        /// <param name="messages"></param>
        public void ProcessResultMessages(List<Message> messages, OperationStatus status)
        {
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    switch (message.Type)
                    {
                        case MessageType.Success:
                            SetSuccessMessage(message.Text, message.Id);
                            break;
                        case MessageType.Information:
                            AddInformationMessage(message.Text, message.Id);
                            break;
                        case MessageType.Warning:
                            AddWarningMessage(message.Text, message.Id);
                            break;
                        case MessageType.Error:
                            AddErrorMessage(message.Text, message.Id);
                            break;
                        default:
                            break;
                    }
                }
            }
            //else if (status == OperationStatus.Success && OperationType != null)
            //{
            //    switch (OperationType)
            //    {
            //        case Common.Enums.OperationType.Delete:
            //            SetSuccessMessage(Localization.SuccessMessage_Delete);
            //            break;
            //        case Common.Enums.OperationType.Save:
            //            SetSuccessMessage(Localization.SuccessMessage_Save);
            //            break;
            //        case Common.Enums.OperationType.Submit:
            //            SetSuccessMessage(Localization.SuccessMessage_Submit);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        public void SetSuccessMessage(string text, string id = null)
        {
            if (Success != null)
                ClearMessages(MessageType.Success);

            var message = new Message(MessageType.Success, id, text);
            Messages.Add(message);
        }
    }
}
