#pragma once
using namespace log4net;
using namespace SqlDataProvider::BaseClass;
using namespace SqlDataProvider::Data;
using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Reflection;
namespace Business
{
	public ref class BaseBusiness
	{
	public:
		BaseBusiness();
		~BaseBusiness();
		DataTable^  GetPage(String^ queryStr, String^ueryWhere, int pageCurrent, int pageSize, String^ fdShow, String^ fdOreder, String^ fdKey, int% total);
		DataTable^ GetFetch_List(int page, int size, String^ sOrder, String^ sWhere, String^ tableName, int% total);

	protected:
		initonly static ILog^ log = LogManager::GetLogger(MethodBase::GetCurrentMethod()->DeclaringType);
		Sql_DbObject^ db;

	};
	public ref class ActiveBusiness : BaseBusiness
	{
	public:
		array<ActiveInfo^>^ GetAllActives();
		ActiveInfo^ GetSingleActives(int activeID);
		ActiveInfo^ InitActiveInfo(SqlDataReader^ reader);
		int PullDown(int activeID, String^ awardID, int userID, String^& string);
		



	};
}

