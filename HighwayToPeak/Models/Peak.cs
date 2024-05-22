using HighwayToPeak.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Models
{
    public class Peak : IPeak
    {
        private string name;
        private int elevation;
        private string difficultyLevel;

        public Peak(string name, int elevation, string difficultyLevel)
        {
            this.Name = name;
            this.Elevation = elevation;
            this.difficultyLevel = difficultyLevel;
        }

        public string Name 
        { 
            get => name; 
            private set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Peak name cannot be null or whitespace.");
                }
                name = value;
            }
        }

        public int Elevation 
        {
            get => elevation; 
           private set
            {
                if(value < 0)
                {
                    throw new ArgumentException("Peak elevation must be a positive value.");
                }
                elevation = value;
            }
        }

        public string DifficultyLevel => difficultyLevel;

        public override string ToString()
        {
            return $"Peak: {Name} -> Elevation: {Elevation}, Difficulty: {DifficultyLevel}";
        }
    }
}
