using L2Monitor.Common.Packets;
using Serilog;
using System.IO;

namespace L2Monitor.Login.Packets.Incoming
{
    public class LoginFail : BasePacket
    {
        // 0x01 - системная ошибка
        // 0x02 - неправельный пароль
        // 0x03 - логин или пароль неверен
        // 0x04 - доступ запрещен
        // 0x05 - информация на аккаунте неверна(хз, наверно имеется ввиду ошибка в БД)
        // 0x07 - аккаунт уже используется
        // 0x09 - аккаунт забанен
        // 0x10 - на сервере идут сервисные работы
        // 0x12 - срок действия истек
        // 0x13 - на аккаунте не осталось больше времени (видимо NCSoft собирается или собиралось заморочить и почасовую оплату :)

        public LoginFail(MemoryStream memStream) : base(memStream)
        {
        }
    }
}
