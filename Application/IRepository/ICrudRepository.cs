using Application.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.CRUDEntities;



namespace Application.IRepository
{
    public interface ICrudRepository
    {

      public (int status, string[] message) StudentDataEntry(StudentData studentData);
        //public List<BloodGroupViewModel> GetBloodGroupList(string organizationCode);

    }
}
