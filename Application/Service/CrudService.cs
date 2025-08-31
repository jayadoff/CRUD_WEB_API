using Application.IRepository;
using Application.IService;
using Domain.Entities.CRUDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class CrudService : ICrudService
    {
        public readonly ICrudRepository _crudRepository;
        public CrudService(ICrudRepository crudRepository)
        {
            _crudRepository = crudRepository;

        }

        public (int status, string[] message) StudentDataEntry(StudentData studentData)
        {
            var StatusAndMsg = _crudRepository.StudentDataEntry(studentData);
            return StatusAndMsg;
        }











    }
}

