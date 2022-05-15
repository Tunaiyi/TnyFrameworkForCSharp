using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Scanner
{

    public class TypeSelectedHandler : ITypeSelectedHandler
    {
        private readonly Action<ICollection<Type>> handler;

        public TypeSelectedHandler(Action<ICollection<Type>> handler)
        {
            this.handler = handler;
        }

        public void Handle(ICollection<Type> types)
        {
            handler(types);
        }
    }

    public interface ITypeSelectedHandler
    {
        void Handle(ICollection<Type> types);
    }

}
