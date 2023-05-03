namespace BlazorApp1.Utils
{
    public class ErrorHandling
    {
        public enum ErrorCodes : int
        {
            ERROR_UNKNOWN = -1,
            ERROR_SUCCESS = 0,

            ERROR_NOT_FOUND = 1,
            ERROR_INVALID_STATUS = 2,
            ERROR_NO_CONTENT = 10,

            ERROR_INVALID_TOKEN = 100,
            ERROR_TOKEN_IS_NOT_WHITELISTED = 101,
            ERROR_TOKEN_HAS_EXPIRED = 102,

            ERROR_INVALID_USER = 200,
            ERROR_USER_NOT_FOUND = 201,
            ERROR_INCORRECT_PASSWORD = 202,
            ERROR_INCORRECT_OLD_PASSWORD = 203,
            ERROR_PASSWORDS_DO_NOT_MATCH = 204,

            ERROR_ORDER_COMPLETE_ONLY_ACTIVE = 602,
            ERROR_ORDER_STATUS_NOT_FOUND = 603,
            ERROR_ORDER_FINAL_STATUS = 604,
            ERROR_ORDER_REASON_NOT_FOUND = 605,
        }
        public static string GetErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case (int)ErrorCodes.ERROR_SUCCESS:
                    return string.Empty;
                    break;

                case (int)ErrorCodes.ERROR_NOT_FOUND:
                    return "Объект не найден";
                    break;

                case (int)ErrorCodes.ERROR_INVALID_STATUS:
                    return "Неверный статус";
                    break;

                case (int)ErrorCodes.ERROR_NO_CONTENT:
                    return "Содержимое пусто";
                    break;

                case (int)ErrorCodes.ERROR_INVALID_TOKEN:
                    return "Неверный токен";
                    break;

                case (int)ErrorCodes.ERROR_TOKEN_IS_NOT_WHITELISTED:
                    return "Токен отсутствует в списке разрешенных";
                    break;

                case (int)ErrorCodes.ERROR_TOKEN_HAS_EXPIRED:
                    return "Срок действия токена истек";
                    break;

                case (int)ErrorCodes.ERROR_INVALID_USER:
                    return "Неверный пользователь";
                    break;

                case (int)ErrorCodes.ERROR_USER_NOT_FOUND:
                    return "Пользователь не найден";
                    break;

                case (int)ErrorCodes.ERROR_INCORRECT_PASSWORD:
                    return "Неверный пароль";
                    break;

                case (int)ErrorCodes.ERROR_INCORRECT_OLD_PASSWORD:
                    return "Старый пароль неверный";
                    break;

                case (int)ErrorCodes.ERROR_PASSWORDS_DO_NOT_MATCH:
                    return "Пароли не совпадают";
                    break;

                case (int)ErrorCodes.ERROR_ORDER_COMPLETE_ONLY_ACTIVE:
                    return "Завершить можно только заказ со статусом OrderStatus.ACTIVE";
                    break;

                case (int)ErrorCodes.ERROR_ORDER_STATUS_NOT_FOUND:
                    return "Статус заказа не найден";
                    break;

                case (int)ErrorCodes.ERROR_ORDER_FINAL_STATUS:
                    return "Невозможно изменить статус у Отмененного или Завершенного заказа";
                    break;

                case (int)ErrorCodes.ERROR_ORDER_REASON_NOT_FOUND:
                    return "Причина отмены не найдена";
                    break;

                default:
                    return "Произошла неизвестная ошибка";
                    break;
            }
        }
    }
}
