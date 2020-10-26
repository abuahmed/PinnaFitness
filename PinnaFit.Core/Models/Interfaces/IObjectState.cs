using System.ComponentModel.DataAnnotations.Schema;


namespace PinnaFit.Core.Models.Interfaces
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
