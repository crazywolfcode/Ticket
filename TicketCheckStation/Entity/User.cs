using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

 namespace TicketCheckStation
{

	 /// <summary>
	 /// 人员信息表
	 /// 数据条数:0
	 /// 数据大小:16KB
	 /// </summary>


	  public  class User
	 {

	 /// <summary>
	 /// 可空:NO
	 /// </summary>

	 public String id{ get; set; }

	 /// <summary>
	 /// 注释:姓名
	 /// 可空:NO
	 /// </summary>

	 public String name{ get; set; }

	 /// <summary>
	 /// 注释:首拼字母
	 /// 可空:YES
	 /// </summary>

	 public String nameFirstCase{ get; set; }

	 /// <summary>
	 /// 注释:身份证号码
	 /// 可空:YES
	 /// </summary>

	 public String idNumber{ get; set; }

	 /// <summary>
	 /// 可空:YES
	 /// </summary>

	 public String phone{ get; set; }

	 /// <summary>
	 /// 注释:生日 
	 /// 可空:YES
	 /// </summary>

	 public DateTime birthday{ get; set; }

	 /// <summary>
	 /// 可空:YES
	 /// </summary>

	 public Int32 sex{ get; set; }

	 /// <summary>
	 /// 可空:YES
	 /// </summary>

	 public String roleId{ get; set; }

	 /// <summary>
	 /// 可空:YES
	 /// </summary>

	 public String roleName{ get; set; }

	 /// <summary>
	 /// 注释:所属站点级别：0验票站，1县组监管中心，2市级 3省级 4国家
	 /// 可空:YES
	 /// </summary>

	 public String pwd{ get; set; }

	 /// <summary>
	 /// 可空:YES
	 /// </summary>

	 public Int32 stationLevelType{ get; set; }

	 /// <summary>
	 /// 注释:备注信息
	 /// 可空:YES
	 /// </summary>

	 public String remark{ get; set; }

	 /// <summary>
	 /// 注释:添加时间
	 /// 可空:NO
	 ///默认值:CURRENT_TIMESTAMP
	 /// </summary>

	 public DateTime addTime{ get; set; }

	 /// <summary>
	 /// 注释:最后变动时间
	 /// 可空:YES
	 /// </summary>

	 public DateTime lastUpdateTime{ get; set; }

	 /// <summary>
	 /// 注释:是否删除 0 否 1 是 
	 /// 可空:NO
	 ///默认值:0
	 /// </summary>

	 public Int32 isDelete{ get; set; }

	 /// <summary>
	 /// 注释:删除时间
	 /// 可空:YES
	 /// </summary>

	 public DateTime deleteTime{ get; set; }

	 /// <summary>
	 /// 注释:最后修改人员Id
	 /// 可空:YES
	 /// </summary>

	 public String lastUpdateUserId{ get; set; }

	 /// <summary>
	 /// 注释:最后修改人姓名
	 /// 可空:YES
	 /// </summary>

	 public String lastUpdateUserName{ get; set; }

	 /// <summary>
	 /// 注释:0 未启用 1 正常启用
	 /// 可空:NO
	 ///默认值:1
	 /// </summary>

	 public Int32 status{ get; set; }

	 /// <summary>
	 /// 注释:添加人信息
	 /// 可空:YES
	 /// </summary>

	 public String addUserId{ get; set; }

	 /// <summary>
	 /// 注释:添加人信息
	 /// 可空:YES
	 /// </summary>

	 public String addUserName{ get; set; }

	 }
}
