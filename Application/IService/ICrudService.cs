using Domain.Entities.CRUDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface ICrudService
    {
     public  (int status, string[] message) StudentDataEntry(StudentData studentData);
        //public List<BloodGroupViewModel> GetBloodGroupList(string organizationCode);

    }
}
