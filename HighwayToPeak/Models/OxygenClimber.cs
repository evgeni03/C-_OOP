using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Models
{
    public class OxygenClimber : Climber
    {
        public OxygenClimber(string name) : base(name, 10)
        {
            // Additional initialization specific to OxygenClimber, if needed
        }
        public override void Rest(int daysCount)
        {
            // OxygenClimber recovers 1 unit of Stamina for every day of rest in the base camp
            Stamina += daysCount;
        }
    }
}