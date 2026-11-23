using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Mvc.ViewModels;

public class JoinGameViewModel
{
    [Required(ErrorMessage = "Joining game is required")]
    [StringLength(4, MinimumLength = 4)]

    public string JoinCode { get; set; } = "";

    [Required(ErrorMessage = "Nickname is required.")]
    [StringLength(20, MinimumLength = 2)]
    public string Nickname { get; set; } = "";
}
