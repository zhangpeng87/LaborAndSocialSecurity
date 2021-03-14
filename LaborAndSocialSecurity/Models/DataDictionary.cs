using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public static class DataDictionary
    {
        static DataDictionary()
        {
            // 职位/工种
            RoleRelationship = new DataTable();
            RoleRelationship.Columns.Add("role_code_pm", typeof(string));
            RoleRelationship.Columns.Add("role_type_pm", typeof(string));
            RoleRelationship.Columns.Add("profession_code_pm", typeof(string));
            RoleRelationship.Columns.Add("profession_type_pm", typeof(string));
            RoleRelationship.Columns.Add("role_code_hj", typeof(string));
            RoleRelationship.Columns.Add("role_type_hj", typeof(string));
            RoleRelationship.Columns.Add("profession_code_hj", typeof(string));
            RoleRelationship.Columns.Add("profession_type_hj", typeof(string));

            object[][] itemArray = new object[126][];
            itemArray[0] = new object[] { "1", "工人", "1", "砌筑工", "01", "务工人员", "010", "砌筑工" };
            itemArray[1] = new object[] { "1", "工人", "2", "钢筋工", "01", "务工人员", "020", "钢筋工" };
            itemArray[2] = new object[] { "1", "工人", "3", "架子工", "01", "务工人员", "030", "架子工" };
            itemArray[3] = new object[] { "1", "工人", "4", "混凝土工", "01", "务工人员", "040", "混凝土工" };
            itemArray[4] = new object[] { "1", "工人", "5", "模板工", "01", "务工人员", "050", "模板工" };
            itemArray[5] = new object[] { "1", "工人", "6", "机械设备安装工", "01", "务工人员", "060", "机械设备安装工" };
            itemArray[6] = new object[] { "1", "工人", "7", "通风工", "01", "务工人员", "070", "通风工" };
            itemArray[7] = new object[] { "1", "工人", "8", "起重工", "01", "务工人员", "080", "安装起重工" };
            itemArray[8] = new object[] { "1", "工人", "9", "安装钳工", "01", "务工人员", "090", "安装钳工" };
            itemArray[9] = new object[] { "1", "工人", "10", "电气设备安装调试工", "01", "务工人员", "100", "电气设备安装调试工" };
            itemArray[10] = new object[] { "1", "工人", "11", "管道工", "01", "务工人员", "110", "管道工" };
            itemArray[11] = new object[] { "1", "工人", "12", "变电安装工", "01", "务工人员", "120", "变电安装工" };
            itemArray[12] = new object[] { "1", "工人", "13", "建筑电工", "01", "务工人员", "130", "建筑电工" };
            itemArray[13] = new object[] { "1", "工人", "14", "司泵工", "01", "务工人员", "140", "司泵工" };
            itemArray[14] = new object[] { "1", "工人", "15", "挖掘铲运和桩工机械司机", "01", "务工人员", "150", "挖掘铲运和桩工机械司机" };
            itemArray[15] = new object[] { "1", "工人", "16", "桩机操作工", "01", "务工人员", "160", "桩机操作工" };
            itemArray[16] = new object[] { "1", "工人", "17", "起重信号工", "01", "务工人员", "170", "起重信号工" };
            itemArray[17] = new object[] { "1", "工人", "18", "建筑起重机械安装拆卸工", "01", "务工人员", "180", "建筑起重机械安装拆卸工" };
            itemArray[18] = new object[] { "1", "工人", "19", "装饰装修工", "01", "务工人员", "190", "装饰装修工" };
            itemArray[19] = new object[] { "1", "工人", "20", "室内成套设施安装工", "01", "务工人员", "200", "室内成套设施安装工" };
            itemArray[20] = new object[] { "1", "工人", "21", "建筑门窗幕墙安装工", "01", "务工人员", "210", "建筑门窗幕墙安装工" };
            itemArray[21] = new object[] { "1", "工人", "22", "幕墙制作工", "01", "务工人员", "220", "幕墙制作工" };
            itemArray[22] = new object[] { "1", "工人", "23", "防水工", "01", "务工人员", "230", "防水工" };
            itemArray[23] = new object[] { "1", "工人", "24", "石工", "01", "务工人员", "250", "石工" };
            itemArray[24] = new object[] { "1", "工人", "25", "泥塑工", "01", "务工人员", "1000", "其它" };
            itemArray[25] = new object[] { "1", "工人", "26", "电焊工", "01", "务工人员", "270", "电焊工" };
            itemArray[26] = new object[] { "1", "工人", "27", "爆破工", "01", "务工人员", "280", "爆破工" };
            itemArray[27] = new object[] { "1", "工人", "28", "除尘工", "01", "务工人员", "290", "除尘工" };
            itemArray[28] = new object[] { "1", "工人", "29", "测量放线工", "01", "务工人员", "300", "测量放线工" };
            itemArray[29] = new object[] { "1", "工人", "30", "线路架设工", "01", "务工人员", "310", "线路架设工" };
            itemArray[30] = new object[] { "1", "工人", "31", "古建筑传统木工", "01", "务工人员", "350", "古建筑传统木工" };
            itemArray[31] = new object[] { "1", "工人", "32", "古建筑传统瓦工", "01", "务工人员", "330", "古建筑传统瓦工" };
            itemArray[32] = new object[] { "1", "工人", "33", "古建筑传统石工", "01", "务工人员", "320", "古建筑传统石工" };
            itemArray[33] = new object[] { "1", "工人", "34", "古建筑传统彩画工", "01", "务工人员", "340", "古建筑传统彩画工" };
            itemArray[34] = new object[] { "1", "工人", "35", "古建筑传统油工", "01", "务工人员", "360", "古建筑传统油工" };
            itemArray[35] = new object[] { "1", "工人", "36", "金属工", "01", "务工人员", "380", "金属工" };
            itemArray[36] = new object[] { "1", "工人", "37", "管理人员", "01", "务工人员", "900", "管理人员" };
            itemArray[37] = new object[] { "1", "工人", "38", "杂工", "01", "务工人员", "390", "杂工" };
            itemArray[38] = new object[] { "1", "工人", "39", "其它", "01", "务工人员", "1000", "其它" };
            itemArray[39] = new object[] { "1", "工人", "40", "木工", "01", "务工人员", "240", "木工" };
            itemArray[40] = new object[] { "1", "工人", "41", "机械司机", "01", "务工人员", "1000", "其它" };
            itemArray[41] = new object[] { "1", "工人", "42", "高级熟练工", "01", "务工人员", "1000", "其它" };
            itemArray[42] = new object[] { "2", "班组长", "1", "砌筑工", "01", "务工人员", "010", "砌筑工" };
            itemArray[43] = new object[] { "2", "班组长", "2", "钢筋工", "01", "务工人员", "020", "钢筋工" };
            itemArray[44] = new object[] { "2", "班组长", "3", "架子工", "01", "务工人员", "030", "架子工" };
            itemArray[45] = new object[] { "2", "班组长", "4", "混凝土工", "01", "务工人员", "040", "混凝土工" };
            itemArray[46] = new object[] { "2", "班组长", "5", "模板工", "01", "务工人员", "050", "模板工" };
            itemArray[47] = new object[] { "2", "班组长", "6", "机械设备安装工", "01", "务工人员", "060", "机械设备安装工" };
            itemArray[48] = new object[] { "2", "班组长", "7", "通风工", "01", "务工人员", "070", "通风工" };
            itemArray[49] = new object[] { "2", "班组长", "8", "起重工", "01", "务工人员", "080", "安装起重工" };
            itemArray[50] = new object[] { "2", "班组长", "9", "安装钳工", "01", "务工人员", "090", "安装钳工" };
            itemArray[51] = new object[] { "2", "班组长", "10", "电气设备安装调试工", "01", "务工人员", "100", "电气设备安装调试工" };
            itemArray[52] = new object[] { "2", "班组长", "11", "管道工", "01", "务工人员", "110", "管道工" };
            itemArray[53] = new object[] { "2", "班组长", "12", "变电安装工", "01", "务工人员", "120", "变电安装工" };
            itemArray[54] = new object[] { "2", "班组长", "13", "建筑电工", "01", "务工人员", "130", "建筑电工" };
            itemArray[55] = new object[] { "2", "班组长", "14", "司泵工", "01", "务工人员", "140", "司泵工" };
            itemArray[56] = new object[] { "2", "班组长", "15", "挖掘铲运和桩工机械司机", "01", "务工人员", "150", "挖掘铲运和桩工机械司机" };
            itemArray[57] = new object[] { "2", "班组长", "16", "桩机操作工", "01", "务工人员", "160", "桩机操作工" };
            itemArray[58] = new object[] { "2", "班组长", "17", "起重信号工", "01", "务工人员", "170", "起重信号工" };
            itemArray[59] = new object[] { "2", "班组长", "18", "建筑起重机械安装拆卸工", "01", "务工人员", "180", "建筑起重机械安装拆卸工" };
            itemArray[60] = new object[] { "2", "班组长", "19", "装饰装修工", "01", "务工人员", "190", "装饰装修工" };
            itemArray[61] = new object[] { "2", "班组长", "20", "室内成套设施安装工", "01", "务工人员", "200", "室内成套设施安装工" };
            itemArray[62] = new object[] { "2", "班组长", "21", "建筑门窗幕墙安装工", "01", "务工人员", "210", "建筑门窗幕墙安装工" };
            itemArray[63] = new object[] { "2", "班组长", "22", "幕墙制作工", "01", "务工人员", "220", "幕墙制作工" };
            itemArray[64] = new object[] { "2", "班组长", "23", "防水工", "01", "务工人员", "230", "防水工" };
            itemArray[65] = new object[] { "2", "班组长", "24", "石工", "01", "务工人员", "250", "石工" };
            itemArray[66] = new object[] { "2", "班组长", "25", "泥塑工", "01", "务工人员", "1000", "其它" };
            itemArray[67] = new object[] { "2", "班组长", "26", "电焊工", "01", "务工人员", "270", "电焊工" };
            itemArray[68] = new object[] { "2", "班组长", "27", "爆破工", "01", "务工人员", "280", "爆破工" };
            itemArray[69] = new object[] { "2", "班组长", "28", "除尘工", "01", "务工人员", "290", "除尘工" };
            itemArray[70] = new object[] { "2", "班组长", "29", "测量放线工", "01", "务工人员", "300", "测量放线工" };
            itemArray[71] = new object[] { "2", "班组长", "30", "线路架设工", "01", "务工人员", "310", "线路架设工" };
            itemArray[72] = new object[] { "2", "班组长", "31", "古建筑传统木工", "01", "务工人员", "350", "古建筑传统木工" };
            itemArray[73] = new object[] { "2", "班组长", "32", "古建筑传统瓦工", "01", "务工人员", "330", "古建筑传统瓦工" };
            itemArray[74] = new object[] { "2", "班组长", "33", "古建筑传统石工", "01", "务工人员", "320", "古建筑传统石工" };
            itemArray[75] = new object[] { "2", "班组长", "34", "古建筑传统彩画工", "01", "务工人员", "340", "古建筑传统彩画工" };
            itemArray[76] = new object[] { "2", "班组长", "35", "古建筑传统油工", "01", "务工人员", "360", "古建筑传统油工" };
            itemArray[77] = new object[] { "2", "班组长", "36", "金属工", "01", "务工人员", "380", "金属工" };
            itemArray[78] = new object[] { "2", "班组长", "37", "管理人员", "01", "务工人员", "900", "管理人员" };
            itemArray[79] = new object[] { "2", "班组长", "38", "杂工", "01", "务工人员", "390", "杂工" };
            itemArray[80] = new object[] { "2", "班组长", "39", "其它", "01", "务工人员", "1000", "其它" };
            itemArray[81] = new object[] { "2", "班组长", "40", "木工", "01", "务工人员", "240", "木工" };
            itemArray[82] = new object[] { "2", "班组长", "41", "机械司机", "01", "务工人员", "1000", "其它" };
            itemArray[83] = new object[] { "2", "班组长", "42", "高级熟练工", "01", "务工人员", "1000", "其它" };
            itemArray[84] = new object[] { "4", "管理人员", "1", "项目负责人", "03", "项目经理", "900", "管理人员" };
            itemArray[85] = new object[] { "4", "管理人员", "2", "项目安全负责人", "12", "安全员", "900", "管理人员" };
            itemArray[86] = new object[] { "4", "管理人员", "3", "总工程师", "09", "工程师", "900", "管理人员" };
            itemArray[87] = new object[] { "4", "管理人员", "4", "安全工程师", "09", "工程师", "900", "管理人员" };
            itemArray[88] = new object[] { "4", "管理人员", "5", "结构工程师", "09", "工程师", "900", "管理人员" };
            itemArray[89] = new object[] { "4", "管理人员", "6", "土建工程师", "09", "工程师", "900", "管理人员" };
            itemArray[90] = new object[] { "4", "管理人员", "7", "水电工程师", "09", "工程师", "900", "管理人员" };
            itemArray[91] = new object[] { "4", "管理人员", "8", "造价工程师", "09", "工程师", "900", "管理人员" };
            itemArray[92] = new object[] { "4", "管理人员", "9", "安装工程师", "09", "工程师", "900", "管理人员" };
            itemArray[93] = new object[] { "4", "管理人员", "10", "土木工程师", "09", "工程师", "900", "管理人员" };
            itemArray[94] = new object[] { "4", "管理人员", "11", "公用设备工程师", "09", "工程师", "900", "管理人员" };
            itemArray[95] = new object[] { "4", "管理人员", "12", "化工工程师", "09", "工程师", "900", "管理人员" };
            itemArray[96] = new object[] { "4", "管理人员", "13", "监理工程师", "09", "工程师", "900", "管理人员" };
            itemArray[97] = new object[] { "4", "管理人员", "14", "消防工程师", "09", "工程师", "900", "管理人员" };
            itemArray[98] = new object[] { "4", "管理人员", "15", "机械工程师", "09", "工程师", "900", "管理人员" };
            itemArray[99] = new object[] { "4", "管理人员", "16", "防护工程师", "09", "工程师", "900", "管理人员" };
            itemArray[100] = new object[] { "4", "管理人员", "17", "测量工程师", "09", "工程师", "900", "管理人员" };
            itemArray[101] = new object[] { "4", "管理人员", "18", "地质工程师", "09", "工程师", "900", "管理人员" };
            itemArray[102] = new object[] { "4", "管理人员", "19", "环保工程师", "09", "工程师", "900", "管理人员" };
            itemArray[103] = new object[] { "4", "管理人员", "20", "项目经理", "03", "项目经理", "900", "管理人员" };
            itemArray[104] = new object[] { "4", "管理人员", "21", "项目副经理", "03", "项目经理", "900", "管理人员" };
            itemArray[105] = new object[] { "4", "管理人员", "22", "技术负责人", "09", "工程师", "900", "管理人员" };
            itemArray[106] = new object[] { "4", "管理人员", "23", "技术员", "09", "工程师", "900", "管理人员" };
            itemArray[107] = new object[] { "4", "管理人员", "24", "施工员", "10", "施工员", "900", "管理人员" };
            itemArray[108] = new object[] { "4", "管理人员", "25", "质量员", "11", "质量员", "900", "管理人员" };
            itemArray[109] = new object[] { "4", "管理人员", "26", "安全员", "12", "安全员", "900", "管理人员" };
            itemArray[110] = new object[] { "4", "管理人员", "27", "标准员", "05", "标准员", "900", "管理人员" };
            itemArray[111] = new object[] { "4", "管理人员", "28", "材料员", "13", "材料员", "900", "管理人员" };
            itemArray[112] = new object[] { "4", "管理人员", "29", "机械员", "06", "机械员", "900", "管理人员" };
            itemArray[113] = new object[] { "4", "管理人员", "30", "劳资专管员", "04", "劳资专管员", "900", "管理人员" };
            itemArray[114] = new object[] { "4", "管理人员", "31", "资料员", "08", "资料员", "900", "管理人员" };
            itemArray[115] = new object[] { "4", "管理人员", "32", "安全负责人", "12", "安全员", "900", "管理人员" };
            itemArray[116] = new object[] { "4", "管理人员", "33", "质量负责人", "11", "质量员", "900", "管理人员" };
            itemArray[117] = new object[] { "4", "管理人员", "34", "项目总监", "14", "其他人员", "900", "管理人员" };
            itemArray[118] = new object[] { "4", "管理人员", "35", "监理员", "14", "其他人员", "900", "管理人员" };
            itemArray[119] = new object[] { "4", "管理人员", "36", "项目专监", "14", "其他人员", "900", "管理人员" };
            itemArray[120] = new object[] { "4", "管理人员", "37", "监理代表", "14", "其他人员", "900", "管理人员" };
            itemArray[121] = new object[] { "4", "管理人员", "38", "建筑设计负责人", "09", "工程师", "900", "管理人员" };
            itemArray[122] = new object[] { "4", "管理人员", "39", "建筑设计工程师", "09", "工程师", "900", "管理人员" };
            itemArray[123] = new object[] { "4", "管理人员", "40", "结构设计工程师", "09", "工程师", "900", "管理人员" };
            itemArray[124] = new object[] { "4", "管理人员", "41", "结构设计负责人", "09", "工程师", "900", "管理人员" };
            itemArray[125] = new object[] { "4", "管理人员", "42", "专班人员", "14", "其他人员", "900", "管理人员" };
            
            DataRow newRow = null;
            foreach (var item in itemArray)
            {
                newRow = RoleRelationship.NewRow();
                RoleRelationship.Rows.Add(newRow);

                newRow.ItemArray = item;
            }

            // 民族
            Nations = new DataTable();
            Nations.Columns.Add("nation_code_hj", typeof(string));
            Nations.Columns.Add("nation_type_hj", typeof(string));

            object[][] nationArray = new object[57][];
            nationArray[0] = new object[] { "01", "汉族" };
            nationArray[1] = new object[] { "02", "蒙古族" };
            nationArray[2] = new object[] { "03", "回族" };
            nationArray[3] = new object[] { "04", "藏族" };
            nationArray[4] = new object[] { "05", "维吾尔族" };
            nationArray[5] = new object[] { "06", "苗族" };
            nationArray[6] = new object[] { "07", "彝族" };
            nationArray[7] = new object[] { "08", "壮族" };
            nationArray[8] = new object[] { "09", "布依族" };
            nationArray[9] = new object[] { "10", "朝鲜族" };
            nationArray[10] = new object[] { "11", "满族" };
            nationArray[11] = new object[] { "12", "侗族" };
            nationArray[12] = new object[] { "13", "瑶族" };
            nationArray[13] = new object[] { "14", "白族" };
            nationArray[14] = new object[] { "15", "土家族" };
            nationArray[15] = new object[] { "16", "哈尼族" };
            nationArray[16] = new object[] { "17", "哈萨克族" };
            nationArray[17] = new object[] { "18", "傣族" };
            nationArray[18] = new object[] { "19", "黎族" };
            nationArray[19] = new object[] { "20", "僳僳族" };
            nationArray[20] = new object[] { "21", "佤族" };
            nationArray[21] = new object[] { "22", "畲族" };
            nationArray[22] = new object[] { "23", "高山族" };
            nationArray[23] = new object[] { "24", "拉祜族" };
            nationArray[24] = new object[] { "25", "水族" };
            nationArray[25] = new object[] { "26", "东乡族" };
            nationArray[26] = new object[] { "27", "纳西族" };
            nationArray[27] = new object[] { "28", "景颇族" };
            nationArray[28] = new object[] { "29", "柯尔克孜族" };
            nationArray[29] = new object[] { "30", "土族" };
            nationArray[30] = new object[] { "31", "达斡尔族" };
            nationArray[31] = new object[] { "32", "仫佬族" };
            nationArray[32] = new object[] { "33", "羌族" };
            nationArray[33] = new object[] { "34", "布朗族" };
            nationArray[34] = new object[] { "35", "撒拉族" };
            nationArray[35] = new object[] { "36", "毛南族" };
            nationArray[36] = new object[] { "37", "仡佬族" };
            nationArray[37] = new object[] { "38", "锡伯族" };
            nationArray[38] = new object[] { "39", "阿昌族" };
            nationArray[39] = new object[] { "40", "普米族" };
            nationArray[40] = new object[] { "41", "塔吉克族" };
            nationArray[41] = new object[] { "42", "怒族" };
            nationArray[42] = new object[] { "43", "乌孜别克族" };
            nationArray[43] = new object[] { "44", "俄罗斯族" };
            nationArray[44] = new object[] { "45", "鄂温克族" };
            nationArray[45] = new object[] { "46", "德昂族" };
            nationArray[46] = new object[] { "47", "保安族" };
            nationArray[47] = new object[] { "48", "裕固族" };
            nationArray[48] = new object[] { "49", "京族" };
            nationArray[49] = new object[] { "50", "塔塔尔族" };
            nationArray[50] = new object[] { "51", "独龙族" };
            nationArray[51] = new object[] { "52", "鄂伦春族" };
            nationArray[52] = new object[] { "53", "赫哲族" };
            nationArray[53] = new object[] { "54", "门巴族" };
            nationArray[54] = new object[] { "55", "珞巴族" };
            nationArray[55] = new object[] { "56", "基诺族" };
            nationArray[56] = new object[] { "57", "穿青人族" };

            newRow = null;
            foreach (var item in nationArray)
            {
                newRow = Nations.NewRow();
                Nations.Rows.Add(newRow);

                newRow.ItemArray = item;
            }

            // 性别字典。
            SexDic = new DataTable();
            SexDic.Columns.Add("sex_code_pm", typeof(string));
            SexDic.Columns.Add("sex_type_pm", typeof(string));
            SexDic.Columns.Add("sex_code_hj", typeof(string));
            SexDic.Columns.Add("sex_type_hj", typeof(string));

            object[][] sexArray = new object[2][];
            sexArray[0] = new object[] { "1", "男", "0", "男" };
            sexArray[1] = new object[] { "2", "女", "1", "女" };

            newRow = null;
            foreach (var item in sexArray)
            {
                newRow = SexDic.NewRow();
                SexDic.Rows.Add(newRow);

                newRow.ItemArray = item;
            }

            // 参建单位类型字典。
            CooperatorTypes = new DataTable();
            CooperatorTypes.Columns.Add("cooperator_code_pm", typeof(string));
            CooperatorTypes.Columns.Add("cooperator_type_pm", typeof(string));
            CooperatorTypes.Columns.Add("cooperator_code_hj", typeof(string));
            CooperatorTypes.Columns.Add("cooperator_type_hj", typeof(string));

            object[][] cooperatorArray = new object[3][];
            cooperatorArray[0] = new object[] { "6", "劳务分包", "03", "劳务分包" };
            cooperatorArray[1] = new object[] { "8", "建设单位", "01", "建设单位" };
            cooperatorArray[2] = new object[] { "9", "总承包单位", "02", "总承包单位" };

            newRow = null;
            foreach (var item in cooperatorArray)
            {
                newRow = CooperatorTypes.NewRow();
                CooperatorTypes.Rows.Add(newRow);

                newRow.ItemArray = item;
            }
        }

        /// <summary>
        /// 0 人员类型编码（品茗）
        /// 1 人员类型名称（品茗）
        /// 2 工种/岗位编码（品茗）
        /// 3 工种/岗位名称（品茗）
        /// 4 人员类型编码（会基）
        /// 5 人员类型名称（会基）
        /// 6 工种/岗位编码（会基）
        /// 7 工种/岗位名称（会基）
        /// </summary>
        public static DataTable RoleRelationship { get; private set; }
        /// <summary>
        /// 民族字典。
        /// </summary>
        public static DataTable Nations { get; private set; }
        /// <summary>
        /// 性别字典。
        /// </summary>
        public static DataTable SexDic { get; private set; }

        public static DataTable CooperatorTypes { get; private set; }
    }
}
