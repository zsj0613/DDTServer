#include "stdafx.h"
#include "Business.h"
using namespace Business;
using namespace System::Collections::Generic;
using namespace System::Data::SqlClient;

array<ActiveInfo^>^ ActiveBusiness::GetAllActives()
{
	List<ActiveInfo^>^ infos = gcnew List<ActiveInfo^>();
	SqlDataReader^ reader = nullptr;
	try
	{
		this->db->GetReader(reader, "SP_Active_All");
		while (reader->Read())
		{
			infos->Add(this->InitActiveInfo(reader));
		}
	}
	catch (Exception^ e)
	{
		BaseBusiness::log->Error("Init", e);
	}
	finally
	{
		if (!reader && !reader->IsClosed)
		{
			reader->Close();
		}
	}
	return infos->ToArray();
}

ActiveInfo^ ActiveBusiness::GetSingleActives(int activeID)
{
	SqlDataReader^ reader = nullptr;
	ActiveInfo^ result;
	try
	{
		array<SqlParameter^>^ para = gcnew array<SqlParameter^>(1);
		para[0] = gcnew SqlParameter("@ID", SqlDbType::Int, 4);
		para[0]->Value = activeID;
		this->db->GetReader(reader, "SP_Active_Single", para);
		if (reader->Read())
		{
			result = this->InitActiveInfo(reader);
			return result;
		}
	}
	catch (Exception^ e)
	{
		BaseBusiness::log->Error("Init", e);
	}
	finally
	{
		if (reader != nullptr && !reader->IsClosed)
		{
			reader->Close();
		}
	}
	result = nullptr;
	return result;
}

ActiveInfo^ ActiveBusiness::InitActiveInfo(SqlDataReader^ reader)
{
	ActiveInfo^ info = gcnew ActiveInfo();
	info->ActiveID = (int)reader->default["ActiveID"];
	info->Description = ((reader->default["Description"] == nullptr) ? "" : (reader->default["Description"]->ToString()));
	info->Content = ((reader->default["Content"] == nullptr) ? "" : reader->default["Content"]->ToString());
	info->AwardContent = ((reader->default["AwardContent"] == nullptr) ? "" : reader->default["AwardContent"]->ToString());
	info->HasKey = (int)reader["HasKey"];

	if (!String::IsNullOrEmpty(reader->default["EndDate"]->ToString()))
	{
		info->EndDate = (DateTime)reader["EndDate"];
	}
	info->IsOnly = (bool)reader["IsOnly"];
	info->StartDate = (DateTime)reader["StartDate"];
	info->Title = reader->default["Title"]->ToString();
	info->Type = (int)reader["Type"];
	info->ActionTimeContent = ((reader->default["ActionTimeContent"] == nullptr) ? "" : reader->default["ActionTimeContent"]->ToString());
	return info;
}

int ActiveBusiness::PullDown(int activeID, String^ awardID, int userID, String^& msg )
{
	int result = 1;
	try
	{
		array<SqlParameter^>^ para= gcnew array<SqlParameter^>
		{
			gcnew SqlParameter("@ActiveID", activeID),
			gcnew SqlParameter("@AwardID", awardID),
			gcnew SqlParameter("@UserID", userID),
			gcnew SqlParameter("@Result", SqlDbType::Int)
		};
		para[3]->Direction = ParameterDirection::ReturnValue;
		if (this->db->RunProcedure("SP_Active_PullDown", para))
		{
			result = (int)para[3]->Value;
			switch (result)
			{
			case 0:
				msg = "ActiveBussiness.Msg0";
				break;
			case 1:
				msg = "ActiveBussiness.Msg1";
				break;
			case 2:
				msg = "ActiveBussiness.Msg2";
				break;
			case 3:
				msg = "ActiveBussiness.Msg3";
				break;
			case 4:
				msg = "ActiveBussiness.Msg4";
				break;
			case 5:
				msg = "ActiveBussiness.Msg5";
				break;
			case 6:
				msg = "ActiveBussiness.Msg6";
				break;
			case 7:
				msg = "ActiveBussiness.Msg7";
				break;
			case 8:
				msg = "ActiveBussiness.Msg8";
				break;
			default:
				msg = "ActiveBussiness.Msg9";
				break;
			}
		}
	}
	catch (Exception^ e)
	{
		BaseBusiness::log->Error("Init", e);
	}
	return result;
}