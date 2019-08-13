using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mappingtester
{
    public class Profile
    {
        private const int DEFAULT_SETS_NUM = 8;

        private string displayName;
        private string description;
        private string author;
        private string fileLoc;
        private int actionSetNum;
        private List<ActionSet> actionSets;
        public ActionSet currentActionSet;

        public Profile(string tempFileLoc)
        {
            actionSets = new List<ActionSet>(DEFAULT_SETS_NUM);
            actionSets.Add(new ActionSet());
            currentActionSet = actionSets[0];
            actionSetNum = 0;
        }

        public Profile(string tempFileLoc, string displayName) :
            this(tempFileLoc)
        {
            this.displayName = displayName;
        }

        public void ChangeActionSet(int index)
        {
            currentActionSet = actionSets[index];
            actionSetNum = index;
        }


        /// <summary>
        /// Load JSON file and make Action instances. Currently does nothing
        /// </summary>
        public void Load()
        {
            /*if (File.Exists(fileLoc))
            {

            }
            */
        }

        public void Release()
        {

        }
    }
}
