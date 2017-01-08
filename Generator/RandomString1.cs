using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altium
{
    public class RandomString1 : IRandomString
    {
        public string GetRandomString()
        {
            var cityRandomizer = StaticRandom.Rand(100000);

            return
                StaticRandom.Rand(999) + ". " +
                (cityRandomizer < Config.predefinedStrings.Length
                ? Config.predefinedStrings[cityRandomizer]
                : Guid.NewGuid().ToString())
                + "\r\n";
            ;
        }
    }
}
