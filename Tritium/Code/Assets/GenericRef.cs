using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Assets
{
    public abstract class GenericRef<T> where T : class
    {
        public string softRef;
        public string softContainer;

        public T resolution;
        protected CargoPackage container;

        public GenericRef()
        {
            softContainer = CargoBay.ActivePackage.id;
        }

        public GenericRef(string path)
        {
            ChangePath(path);
        }

        public virtual T Resolve()
        {
            if (resolution != null)
                return resolution;
            else
            {
                resolution = FetchReference();
                return resolution;
            }
        }

        protected abstract T FetchReference();

        public void ChangePath(string path)
        {
            if (!path.StartsWith('/'))
            {
                var slash = path.IndexOf('/');

                softRef = path.Substring(slash);
                var id = path.Substring(0, slash);

                softContainer = id;
            }
            else
            {
                softRef = path;
                softContainer = CargoBay.ActivePackage.id;
            }
        }

        protected void ResolveContainer()
        {
            if (container == null)
                container = CargoBay.LoadedPackages[softContainer];

        }

        protected string GetPath()
        {
            ResolveContainer();
            string root = CargoBay.LoadedPackageFolders[softContainer];
            string subfolder = container.contentPath;

            return root + subfolder + softRef;
        }

        // Note: C# won't inherit this sometimes for an unknown reason!
        public static implicit operator T(GenericRef<T> r) => r.Resolve() as T;
    }
}
