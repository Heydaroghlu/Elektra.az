using System.Globalization;
using OCPP.Core.Application.DTOs.CPointDTOs;

namespace OCPP.Core.Infrastructure.Services.StringService;

public class PropService
{
    public OnlineConnector ParseData(string data)
    {
        OnlineConnector messageDto = new OnlineConnector();

        // Split the string by the separator '|'
        var parts = data.Split('|');

        foreach (var part in parts)
        {
            // Trim whitespace and split by ':'
            var keyValue = part.Trim().Split(':');

            if (keyValue.Length == 2)
            {
                var key = keyValue[0].Trim();
                var valueString = keyValue[1].Trim();

                if (double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    if (key.StartsWith("Meter"))
                    {
                        messageDto.Meter = value;
                    }
                    else if (key.StartsWith("Charge"))
                    {
                        messageDto.Charge = value;
                    }
                    else if (key.StartsWith("SoC"))
                    {
                        messageDto.SoC = value;
                    }
                }
                else
                {
                    // Handle parsing error
                    Console.WriteLine($"Parsing error for key: {key}, value: {valueString}");
                }
            }
        }

        return messageDto;
    }
}

