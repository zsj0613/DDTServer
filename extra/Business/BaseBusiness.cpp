#include "stdafx.h"
#include "Business.h"
using namespace Business;
using namespace System::Reflection;
using namespace System::Data::SqlClient;
using namespace SqlDataProvider::BaseClass;
using namespace log4net;


BaseBusiness::BaseBusiness()
{
	this->db = gcnew Sql_DbObject("AppConfig", "conString");
}
BaseBusiness::~BaseBusiness()
{
	delete this->db;
	GC::SuppressFinalize(this);
}

DataTable^ BaseBusiness::GetPage(String^ queryStr, String^ queryWhere, int pageCurrent, int pageSize, String^ fdShow, String^ fdOreder, String^ fdKey, int% total)
{
	DataTable^ result;
	try
	{
		array<SqlParameter^ >^ para = gcnew array<SqlParameter^ >
		{
			gcnew SqlParameter("@QueryStr", queryStr),
			gcnew SqlParameter("@QueryWhere", queryWhere),
			gcnew SqlParameter("@PageSize", pageSize),
			gcnew SqlParameter("@PageCurrent", pageCurrent),
			gcnew SqlParameter("@FdShow", fdShow),
			gcnew SqlParameter("@FdOrder", fdOreder),
			gcnew SqlParameter("@FdKey", fdKey),
			gcnew SqlParameter("@TotalRow", total)
		};
		para[7]->Direction = ParameterDirection::Output;
		DataTable^ dt = this->db->GetDataTable(queryStr, "SP_CustomPage", para, 120);
		total = (int)para[7]->Value;
		result = dt;
		return result;
	}
	catch (Exception^ e)
	{
		BaseBusiness::log->Error((Object^)String::Format("Custome Page queryStr@{0},queryWhere@{1},pageCurrent@{2},pageSize@{3},fdShow@{4},fdOrder@{5},fdKey@{6}", gcnew array<Object^ >
		{
				queryStr,
				queryWhere,
				pageCurrent,
				pageSize,
				fdShow,
				fdOreder,
				fdKey
		}), e);
	}
	finally
	{
	}
	result = gcnew DataTable(queryStr);
	return result;
}

DataTable^ BaseBusiness::GetFetch_List(int page, int size, String^ sOrder, String^ sWhere, String^ tableName, int% total)
{
	DataTable^ result;
	try
	{
		array<SqlParameter^>^ para = gcnew array<SqlParameter^>(6);
		para[0] = gcnew SqlParameter("@page_num", page);
		para[1] = gcnew SqlParameter("@row_in_page", size);
		para[2] = gcnew SqlParameter("@order_column", sOrder);
		para[3] = gcnew SqlParameter("@row_total", total);
		para[3]->Direction = ParameterDirection::Output;
		para[4] = gcnew SqlParameter("@comb_condition", sWhere);
		para[5] = gcnew SqlParameter("@tablename", tableName);
		DataTable^ dt = this->db->GetDataTable("table1", "Sp_Fetch_List", para);
		total = (int)para[3]->Value;
		result = dt;
		return result;
	}
	catch (Exception^ e)
	{
		BaseBusiness::log->Error(String::Format("Sp_Fetch_List  page@{0},sOrder@{1},sWhere{2},tableName{3}", gcnew array<Object^>
			{
				page,
				size,
				sOrder,
				sWhere,
				tableName
			}), e);
	}
	finally
	{
	}
	result = gcnew DataTable(tableName);
	return result;
}

