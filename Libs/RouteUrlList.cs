namespace Swagger2Doc.Libs
{
    public sealed class RouteUrlList
    {
        public string Path { get; set; }

        public string Description { get; set; }

        public RouteUrlList(string path, string description)
        {
            Path = path;
            Description = description;
        }

        #region Pages
        public static RouteUrlList ErrorPage = new RouteUrlList("/ErrorPage", "錯誤頁面");

        public static RouteUrlList Swagger = new RouteUrlList("/Swagger", "任務驗證授權");

        public static RouteUrlList Index = new RouteUrlList("/Index", "SignalR 版本的Dashboard");

        public static RouteUrlList IndexApi = new RouteUrlList("/IndexApi", "Api 輪詢版本的Dashboard");

        public static RouteUrlList Role = new RouteUrlList("/Role", "任務驗證授權");
        #endregion

        #region LogAPIs
        public static RouteUrlList LogAPI_GetLogFileList = new RouteUrlList("/LogApi/GetLogFileList", "取得project log files");
        #endregion

        #region RoleAPIs Role管理【IP驗證與工作授權】
        public static RouteUrlList RoleAPI_GetAllJobs = new RouteUrlList("/RoleApi/GetAllJobs", "取得所有工作");

        public static RouteUrlList RoleAPI_GetAllRoleDetail = new RouteUrlList("/RoleApi/GetAllRoleDetail", "取得所有 Role (含分頁功能)");

        public static RouteUrlList RoleAPI_GetRoleDetailByRoleName = new RouteUrlList("/RoleApi/GetRoleDetailByRoleName", "取得特定 Role 資訊(給編輯功能用)");

        public static RouteUrlList RoleAPI_InsertRole = new RouteUrlList("/RoleApi/InsertRole", "新增 Role and Ips");

        public static RouteUrlList RoleAPI_Delete = new RouteUrlList("/RoleApi/Delete", "刪除 Role");

        public static RouteUrlList RoleAPI_ModifyRole = new RouteUrlList("/RoleApi/ModifyRole", "更新 工作");
        #endregion

        #region SchedulerAPIs 排程管理APIs Dashboard api區塊
        public static RouteUrlList SchedulerAPI_GetAllJobLog = new RouteUrlList("/SchedulerApi/JobLog/All", "取得所有Log");

        public static RouteUrlList SchedulerAPI_GetJobLogByName = new RouteUrlList("/SchedulerApi/JobLog", "取得Log By Name");

        public static RouteUrlList SchedulerAPI_GetSchedulerLog = new RouteUrlList("/SchedulerApi/SchedulerLog", "取得所有Log");

        public static RouteUrlList SchedulerAPI_GetJobDetailByName = new RouteUrlList("/SchedulerApi/GetJobDetailByName", "取得單筆 Job DB 設定檔與細節");

        public static RouteUrlList SchedulerAPI_GetJobAccessRuleAndIpByJobId = new RouteUrlList("/SchedulerApi/GetJobAccessRuleAndIpByJobId", "取得 取得 某項工作 授權角色");

        public static RouteUrlList SchedulerAPI_ModifyJobDetail = new RouteUrlList("/SchedulerApi/ModifyJobDetail", "更新 排程任務");

        public static RouteUrlList SchedulerAPI_InsertJob = new RouteUrlList("/SchedulerApi/InsertJob", "新增工作");

        #endregion

        #region SchedulerAPIs 排程管理APIs 排程控制api區塊
        public static RouteUrlList SchedulerAPI_GetAllJobStatus = new RouteUrlList("/SchedulerApi/GetAllJobStatus", "要求取得所有排程Job狀態 與 排程器 是否執行中");

        public static RouteUrlList SchedulerAPI_GetJobStatusByName = new RouteUrlList("/SchedulerApi/GetJobStatusByName", "取得目前排程工作 狀態與資訊");

        public static RouteUrlList SchedulerAPI_TriggerJob = new RouteUrlList("/SchedulerApi/TriggerJob", "手動觸發Job執行");

        public static RouteUrlList SchedulerAPI_InterruptJob = new RouteUrlList("/SchedulerApi/InterruptJob", "手動中斷Job執行");

        public static RouteUrlList SchedulerAPI_DeleteJob = new RouteUrlList("/SchedulerApi/DeleteJob", "刪除Job");

        public static RouteUrlList SchedulerAPI_StartScheduler = new RouteUrlList("/SchedulerApi/StartScheduler", "開啟排程器");

        public static RouteUrlList SchedulerAPI_StopScheduler = new RouteUrlList("/SchedulerApi/StopScheduler", "關閉排程器");
        #endregion
    }
}
