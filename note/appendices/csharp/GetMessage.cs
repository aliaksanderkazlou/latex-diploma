[HttpPost("")]
public async Task<IActionResult> GetMessage([FromBody]Update update)
{
    var message = update.Message;

    if (update.Message == null && update.CallbackQuery == null)
    {
        return Ok();
    }

    var messageToProcess = message != null
        ? ControllerHelper.GetMessageFromMessageStructure(message)
        : ControllerHelper.GetMessageFromUpdateStructure(update);
        
    var responseList =
        await _commandService.ExecuteCommand(messageToProcess.Text.Split(' ')[0], messageToProcess);

        foreach (var response in responseList)
        {
            if (response.StatusCode == StatusCodeEnum.NeedKeyboard)
            {
                await _botClient.SendTextMessageAsync(messageToProcess.UserInfo.ChatId,
                    response.Message,
                    replyMarkup: Helpers.ControllerHelper.BuildKeyBoardMarkup((List<string>) response.Helper));
            }
            else
            {
                await _botClient.SendTextMessageAsync(messageToProcess.UserInfo.ChatId, response.Message);
            }
        }

    return Ok();
}