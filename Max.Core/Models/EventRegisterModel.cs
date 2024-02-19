using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EventRegisterModel
    {
        public int EventRegisterId { get; set; }
        public int EventId { get; set; }
        public int EntityId { get; set; }
        public List<SelectedSessions> SelectedSession { get; set; }
        public List<SelectedQuestions> SelectedQuestion { get; set; }
        public int CurrentUserId { get; set; }
        public EventRegisterModel()
        {
            SelectedSession = new List<SelectedSessions>();
            SelectedQuestion = new List<SelectedQuestions>();
        }
    }

    public class SelectedSessions
    {
        public int SessionId { get; set; }
        public decimal Price { get; set; }
    }

    public class SelectedQuestions
    {
        public int SessionId { get; set; }
        public int EventId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerValue { get; set; }
        public List<int> SelectedAnwserOptions { get; set; }
        public List<AnswerOptionList> AnswerOption { get; set; }
    }

    public class AnswerOptionList
    {
        public int AnswerOptionId { get; set; }
        public int QuestionBankId { get; set; }
        public string Option { get; set; }
    }
}
