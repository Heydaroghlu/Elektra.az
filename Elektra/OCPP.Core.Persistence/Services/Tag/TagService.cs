using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Persistence.Services.Tag
{
    public class TagService
    {
        public  string GenerateRandomCode()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            Random random = new Random();
            char firstChar = letters[random.Next(letters.Length)];
            string remainingChars = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                remainingChars += numbers[random.Next(numbers.Length)];
            }
            return firstChar + remainingChars;
        }
    }
}
