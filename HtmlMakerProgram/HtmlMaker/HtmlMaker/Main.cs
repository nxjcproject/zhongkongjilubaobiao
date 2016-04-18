using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlMaker
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
  

        private void button1_Click(object sender, EventArgs e)
        {

            string title = MainTitle.Text;    //标题
            string mudulas = Modulars.Text;
            mudulas = mudulas.Replace('\t', ' ').Replace('\n', ' ');
            string fields=  Fields.Text;
            fields = fields.Replace('\t', ' ').Replace('\n', ' ');
            string units = Units.Text;
            units = units.Replace('\t', ' ').Replace('\n', ' ');

            StringBuilder sb=new StringBuilder();
            sb.Append("<table id=\"RecordTable\" class=\"table\">\n <caption style=\"font-size: 14.0pt; font-family: 宋体;\"><b>" + title + "</b></caption>\n");//标题完成
            //添加第一层
            sb.Append("<tr>\n<td class=\"td\" rowspan=\"3\"style=\"text-align: center; font-size: 12.0pt; font-family: 宋体;width:25px\"><b>班<br />次</b></td>\n");
            sb.Append("<td class=\"td\" rowspan=\"3\" style=\"text-align: center; font-size: 12.0pt; font-family: 宋体;width:25px\"><b>时<br />间</b></td>\n");
            int AcolNum = new int();
            string[] mudula = mudulas.Split('，');
            for (int i = 0; i < mudula.Length;i++ )
            {
              string[] col= mudula[i].ToString().Split(' ');
              string  colName=col[0].ToString();
              int colCount=Convert.ToInt16(col[1]);
              AcolNum = AcolNum + colCount;
                // <td colspan="2" style="text-align:center">分块一</td>
              sb.Append("<td class=\"td\" colspan=\"" + colCount + "\" style=\"text-align: center; font-size: 12.0pt; font-family: 宋体; \"><b>" + colName + "</b></td>\n");
            
            }
           


            sb.Append(" </tr>\n"); //第一层添加完成

            //第二层
            sb.Append("<tr>\n");
            string[] field = fields.Split(new char[2] { ' ', '\t' });
            for (int m = 0; m < field.Length;m++ )
            {
                string f= field[m].Trim();
                string  fHtml="";
                foreach(char c in f){
                    fHtml =fHtml+ c + "<br/>";             
                }
                sb.Append("<td class=\"td\" style=\"text-align: center; font-size: 10.0pt; font-family: 宋体;width:26px\"><b>" + fHtml + "</b></td>\n");
            }
            sb.Append("</tr>\n");

            //加载第三层
           sb.Append("<tr>\n");
           string[] unit = units.Split(new char[2] { ' ', '\t' });
           for (int n = 0; n < unit.Length; n++)
           {
               string u  = unit[n].Trim();
               sb.Append("<td class=\"td\" style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman'\"><b>" + u + "</b></td>\n");
           }
           sb.Append("</tr>\n");


           string tdList = "";
           for (int A = 0; A<AcolNum; A++)
           {
               tdList = tdList + "<td class=\"td\"></td>";
           }
            //style=\"width:18px\"

            //夜班
           sb.Append(" <tr>\n<td class=\"td\" rowspan=\"8\" style=\"text-align: center; font-size: 12.0pt; font-family: 宋体;\"><b>夜<br />班<br /></b></td>");
           sb.Append("<td id=\"0\" class=\"td\" style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>0</b></td>");
           sb.Append(tdList+"</tr>\n");

           StringBuilder trListNight = new StringBuilder();           
           for (int AN = 1; AN < 8; AN++)
           {
               trListNight.Append("<tr>\n");
               trListNight.Append(" <td class=\"td\" id=\"" + AN + "\"style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>" + AN + "</b></td>");
               trListNight.Append(tdList + "</tr>\n");
           }
           sb.Append(trListNight);

           //白班
           sb.Append(" <tr>\n<td class=\"td\" rowspan=\"8\" style=\"text-align: center; font-size: 12.0pt; font-family: 宋体;\"><b>白<br />班<br /></b></td>");
           sb.Append("<td class=\"td\" id=\"8\"style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>8</b></td>");
           sb.Append(tdList + "</tr>\n");

           StringBuilder trListDay = new StringBuilder();
           for (int AD = 9; AD < 16; AD++)
           {
               trListDay.Append("<tr>\n");
               trListDay.Append(" <td class=\"td\" id=\"" + AD + "\"style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>" + AD + "</b></td>");
               trListDay.Append(tdList + "</tr>\n");
           }
           sb.Append(trListDay);
            //中午
           sb.Append(" <tr>\n<td class=\"td\" rowspan=\"8\" style=\"text-align: center; font-size: 12.0pt; font-family: 宋体;\"><b>中<br />班<br /></b></td>");
           sb.Append("<td class=\"td\" id=\"16\"style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>16</b></td>");
           sb.Append(tdList + "</tr>\n");

           StringBuilder trListNoon = new StringBuilder();
           for (int AN = 17; AN < 24; AN++)
           {
               trListNoon.Append("<tr>\n");
               trListNoon.Append(" <td class=\"td\" id=\"" + AN + "\"style=\"text-align: center;font-size: 10.0pt; font-family: 'Times New Roman';width:18px\"><b>" + AN + "</b></td>");
               trListNoon.Append(tdList + "</tr>\n");
           }
           sb.Append(trListNoon);

            //增加最后一行
           sb.Append("<tr>\n");
           sb.Append("<td class=\"td\" rowspan=\"1\"style=\"height:24px\"></td>\n<td class=\"td\"></td>\n");
           sb.Append(tdList+"</tr>\n");

           sb.Append("</table>");
           string _htmlcode = sb.ToString();
           codeout.Text = _htmlcode;
        }

    }
}
