using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.DTOs;

public class SubmitAnswerRequest
{
    [Range(1, int.MaxValue)]
    public int PlayerId { get; set; }

    [Range(1, int.MaxValue)]
    public int AnswerId { get; set; }
}
