using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ConsoleAuto.Services
{
    public class InternalServiceCollection : IServiceCollection
    {

        Dictionary<int, ServiceDescriptor> services = new Dictionary<int, ServiceDescriptor>();
        public ServiceDescriptor this[int index] { get 
            {
                return services[index];
            } 
            set  
            {
                services[index] = value;
            } 
        }

        public int Count => services.Keys.Count;

        public bool IsReadOnly => false;

        public void Add(ServiceDescriptor item)
        {
            services.Add(services.Count,item);
        }

        public void Clear()
        {
            this.services.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return this.services.Values.Any(x => x.Equals(item));
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            this.services.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
           return  this.services.Values.GetEnumerator();
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return this.services.Where(x => x.Value == item).Select(x => x.Key).FirstOrDefault();
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            this.services.Add(index, item);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return this.services.Remove(IndexOf(item));
        }

        public void RemoveAt(int index)
        {
             this.services.Remove(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.services.GetEnumerator();
        }

        
    }
}
