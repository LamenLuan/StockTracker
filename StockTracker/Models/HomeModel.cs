using StockTracker.Models.Shared.Form;
using StockTracker.Models.Shared.Form.Inputs;
using StockTracker.ViewModels;
using StockTrackerConfigurator.Models;

namespace StockTracker.Models
{
  public class HomeModel : FormModel
  {
    public const string API_KEY_FORM_ID = "brapi-key-form";
    public const string CARDS_ID = "cards";

    public const string API_KEY_INPUT_NAME = "api-key-input";

    public List<CreationCardModel> Cards { get; set; }

    public HomeModel(List<CreationCardViewModel> cardsViewModel, bool hasKey)
    {
      Id = API_KEY_FORM_ID;
      Inputs = CreateInputs(hasKey);
      FormButtonModel = null;
      Cards = [.. cardsViewModel.Select(c => new CreationCardModel(c))];
    }

    private static InputModel[] CreateInputs(bool hasKey)
    {
      return
      [
        new PasswordInputModel
        {
          Label = "Brapi key",
          Name = API_KEY_INPUT_NAME,
          IsRequired = true,
          HasContent = hasKey
        }
      ];
    }
  }
}
