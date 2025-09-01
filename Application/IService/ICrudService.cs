using Domain.Entities.CRUDEntities;

namespace Application.IService
{
    public interface ICrudService
    {
     public  (int status, string[] message) StudentDataEntry(StudentData studentData);
        //public List<BloodGroupViewModel> GetBloodGroupList(string organizationCode);

    }
}
