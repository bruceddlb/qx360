//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//	   生成时间 2018-07-18 15:21:41 by bruced
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
    
using System;
namespace QX360.Model
{
    /// <summary>
    /// 数据表实体类：TagEntity 
    /// </summary>
    [Serializable()]
    public partial class TagEntity:BaseModel
    {    
		            
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string TagId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string ObjectId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string ObjectName { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string Value { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? Type { get; set; }

                    
		/// <summary>
		/// datetime:
		/// </summary>	
                 
		public DateTime? CreateTime { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string CreateId { get; set; }

           
    }    
}
	