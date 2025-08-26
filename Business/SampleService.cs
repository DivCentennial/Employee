using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Business
{
    public interface ISampleService
    {
        string GetData();
    }

    public class SampleService : ISampleService
    {
        private readonly ISampleRepository _sampleRepository;

        public SampleService(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }

        public string GetData()
        {
            var result = _sampleRepository.GetData();

            return result;
        }
    }
}
