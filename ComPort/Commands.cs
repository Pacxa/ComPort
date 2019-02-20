using System;
using System.Collections.Generic;
using System.Linq;

namespace ComPort
{
    class Commands
    {
        string fatalKSA = "";
        string flagKSA = "";
        string docStatus = "";
        string flagSKNO = "";
        string status = "";
        string typeDoc = "";
        string statDoc = "";
        static char[] arr;
        static char[] arr2;
        int arrLen;
        int p = 1;

        public string statusKSA(string result)
        {
            MainWindow main = new MainWindow();
            status = "";
            try
            {
                string[] data = result.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //------------------------------------------------------------------------------------------------------------------------------fatalKSA
                fatalKSA = ReverseString(Convert.ToString(Int32.Parse(data[0].Substring(4, 2)), 2));
                arr = fatalKSA.ToCharArray();
                arrLen = arr.Length;
                Array.Resize(ref arr, 7);

                while (arr.Length != 7)
                {
                    arr[arrLen + p] = '0';
                    p++;
                }

                if (arr[0] == '1') { status += "Неверная контрольная сумма NVR \n"; }
                if (arr[1] == '1') { status += "Неверная контрольная сумма в конфигурации \n"; }
                if (arr[2] == '1') { status += "Ошибка интерфейса с БЭП \n"; }
                if (arr[3] == '1') { status += "Неверная контрольная сумма блока энергонезависимой памяти \n"; }
                if (arr[4] == '1') { status += "Ошибка записи в БЭП \n"; }
                if (arr[5] == '1') { status += "БЭП не авторизован \n"; }
                if (arr[6] == '1') { status += "Фатальная ошибка памяти контрольной ленты \n"; }
                //------------------------------------------------------------------------------------------------------------------------------flagKSA
                flagKSA = ReverseString(Convert.ToString(Int32.Parse(data[1].ToString()), 2));
                arr = flagKSA.ToCharArray();
                arrLen = arr.Length;
                Array.Resize(ref arr, 9);

                while (arr.Length != 9)
                {
                    arr[arrLen + p] = '0';
                    p++;
                }
                if (arr[0] == '1') { status += "Не была вызвана функция 'Начало работы' \n"; }
                if (arr[1] == '1') { status += "Учебный режим \n"; }
                if (arr[2] == '1') { status += "Смена открыта \n"; } else { status += "Смена закрыта \n"; }
                if (arr[3] == '1') { status += "Смена больше 24 часов \n"; }
                if (arr[4] == '1') { status += "Присутствуют данные в буфере контрольной ленты (буфер не пустой) \n"; }
                if (arr[5] == '1') { status += "Зарезервирован \n"; }
                if (arr[6] == '1') { status += "Нет памяти для закрытия смены в БЭП \n"; }
                if (arr[7] == '1') { status += "Зарезервирован \n"; }
                if (arr[8] == '1') { status += "Не было завершено закрытие смены, необходимо повторить операцию \n"; }
                //------------------------------------------------------------------------------------------------------------------------------docStatus
                docStatus = Convert.ToString(Int32.Parse(data[2]), 2);
                while (docStatus.Length != 8)
                {
                    docStatus = "0" + docStatus;
                }
                byte[] rr = main.stringToByte(docStatus);

                typeDoc = rr[0].ToString() + rr[1].ToString() + rr[2].ToString() + rr[3].ToString();
                typeDoc = typeDoc.Replace("48", "0").Replace("49", "1");
                typeDoc = Convert.ToInt32(typeDoc, 2).ToString();

                statDoc = rr[4].ToString() + rr[5].ToString() + rr[6].ToString() + rr[7].ToString();
                statDoc = statDoc.Replace("48", "0").Replace("49", "1");
                statDoc = Convert.ToInt32(statDoc, 2).ToString();

                if (statDoc == "0") { status += "Статус документа: Документ закрыт \n"; }
                if (statDoc == "1") { status += "Статус документа: Сервисный документ \n"; }
                if (statDoc == "2") { status += "Статус документа: Чек на продажу \n"; }
                if (statDoc == "3") { status += "Статус документа: Чек на возврат \n"; }
                if (statDoc == "4") { status += "Статус документа: Внесение в кассу \n"; }
                if (statDoc == "5") { status += "Статус документа: Инкассация \n"; }

                if (typeDoc == "0") { status += "Тип документа: Документ закрыт \n"; }
                if (typeDoc == "1") { status += "Тип документа: Ввод позиции \n"; }
                if (typeDoc == "2") { status += "Тип документа: Ввод скидки \n"; }
                if (typeDoc == "3") { status += "Тип документа: Ввод оплаты \n"; }
                if (typeDoc == "4") { status += "Тип документа: Расчет завершен – требуется закрыть документ \n"; }

                //------------------------------------------------------------------------------------------------------------------------------flagSKNO
                flagSKNO = ReverseString(Convert.ToString(Int32.Parse(data[3].ToString()), 2));
                arr = flagSKNO.ToCharArray();
                arrLen = arr.Length;
                Array.Resize(ref arr, 8);

                while (arr.Length != 8)
                {
                    arr[arrLen + p] = '0';
                    p++;
                }
                if (arr[0] == '1') { status += "Общая ошибка СКНО \n"; }
                if (arr[1] == '1') { status += "Отсутствует связь с СКНО \n"; }
                if (arr[2] == '1') { status += "Отсутствует СКЗИ \n"; }
                if (arr[3] == '1') { status += "Неисправно СКНО \n"; }
                if (arr[4] == '1') { status += "Идентификация не прошла успешно \n"; }
                if (arr[5] == '1') { status += "Запрет обслуживания по окончанию сертификата СКЗИ \n"; }
                if (arr[6] == '1') { status += "Запрет обслуживания по непереданным Z-отчетам \n"; }
                if (arr[7] == '1') { status += "Запрет обслуживания по переполнению памяти СКНО \n"; }
            } catch (Exception ex) { main.statKSA.Items.Add(ex.Message); }
            return status;
        }

        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
