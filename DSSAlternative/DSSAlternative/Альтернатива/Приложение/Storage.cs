using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.JSInterop;
using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    interface IStorage
    {
        bool Login(User user, string password);

        void SaveTemplates(IEnumerable<ITemplate> templates);
        IEnumerable<ITemplate> GetTemplates();
        void UpdateTemplate(ITemplate template);
        void RemoveTemplate(ITemplate template);

        void SaveDssState(DssState state);
        IEnumerable<ITemplate> GetState();
    }
    public class Storage : IStorage
    {
        public Storage()
        {

        }
        public IEnumerable<ITemplate> GetState()
        {
            throw new NotImplementedException();
        }

        public bool Login(User user, string password)
        {
            return true;
        }
        public IEnumerable<ITemplate> GetTemplates()
        {
            throw new NotImplementedException();
        }
        public void RemoveTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }
        public void SaveDssState(DssState state)
        {
            throw new NotImplementedException();
        }
        public void SaveTemplates(IEnumerable<ITemplate> templates)
        {
            throw new NotImplementedException();
        }
        public void UpdateTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }
    }

}
