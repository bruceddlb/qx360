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
    /// 数据表实体类：ArticleEntity 
    /// </summary>
    [Serializable()]
    public partial class ArticleEntity:BaseModel
    {    
		            
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string ArticleId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string Title { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string SortDesc { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string Link { get; set; }

                    
		/// <summary>
		/// text:
		/// </summary>	
                 
		public string Content { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? Status { get; set; }

                    
		/// <summary>
		/// text:
		/// </summary>	
                 
		public string ToGroupName { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string ToGroup { get; set; }

                    
		/// <summary>
		/// datetime:
		/// </summary>	
                 
		public DateTime? CreateDate { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string CreateId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string CreateName { get; set; }

           
    }    
}
	