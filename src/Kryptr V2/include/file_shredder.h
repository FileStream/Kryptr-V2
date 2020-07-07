#include <iostream>
#include <sstream>
#include <fstream>
#include <string>
#include <vector>
#include <cstdio>
#include <cstdlib>
#include <filesystem>
#include "exceptions.h"
#include "fileapi.h"
#include "ioapiset.h"
#include "winioctl.h"
#include "colors.h"

using namespace std::filesystem;

class FileShredder {

	// Overwrite the input file with random data
	// Returns true if successful, false otherwise
private:

	static bool WriteRandomData(const path& inputPath)
		{
		std::error_code ec;
		const auto inputFileSize = file_size(inputPath, ec);
		if (ec)
		{
			return false;
		}

		std::ofstream fout(inputPath.string(), std::ios::binary);
		if (!fout)
		{
			return false;
		}

		std::vector<unsigned char> buffer(inputFileSize);

		////////////////////////////////////////////////////////////////////
		// Overwrite file with ASCII 255 and ASCII 0 multiple times alternately
		int iterations = 5;
		while (iterations--)
		{
			const unsigned char c = (iterations & 1) ? 255 : 0;
			for (auto& bufferElement : buffer)
			{
				bufferElement = c;
			}
			fout.seekp(0, std::ios::beg);
			fout.write((char*)& buffer[0], buffer.size());
			fout.flush();
		}
		////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////
		// Overwrite file with random characters
		for (auto& bufferElement : buffer)
		{
			bufferElement = rand() % 128;
		}
		fout.seekp(0, std::ios::beg);
		fout.write((char*)& buffer[0], buffer.size());
		fout.flush();
		////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////
		// Overwrite file with null characters
		for (auto& bufferElement : buffer)
		{
			bufferElement = 0;
		}
		fout.seekp(0, std::ios::beg);
		fout.write((char*)& buffer[0], buffer.size());
		fout.flush();
		////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////
		// Change file size to 0
		fout.close();
		fout.open(inputPath.string(), std::ios::binary);
		////////////////////////////////////////////////////////////////////

		fout.close();
		return true;
	}

	//Use IOCTL storage query to determine if drive is SSD
	static bool DataOnSSD(const path& inputPath) {
		std::string driveLetter = std::string("\\\\.\\") + inputPath.string().substr(0, 2);
		HANDLE driveHandle = CreateFileA(driveLetter.c_str(), 0, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0); //Query the handle for the drive
		if (driveHandle != INVALID_HANDLE_VALUE) {

			STORAGE_PROPERTY_QUERY spqTrim;
			spqTrim.PropertyId = STORAGE_PROPERTY_ID::StorageDeviceTrimProperty;
			spqTrim.QueryType = STORAGE_QUERY_TYPE::PropertyStandardQuery;

			DEVICE_TRIM_DESCRIPTOR dtd = { 0 };
			DWORD bytesReturned = 0;
			DeviceIoControl(
				driveHandle,                 // handle to device
				IOCTL_STORAGE_QUERY_PROPERTY, // dwIoControlCode
				&spqTrim,                             // lpInBuffer
				sizeof(spqTrim),                                // nInBufferSize
				&dtd,             // output buffer
				sizeof(DEVICE_TRIM_DESCRIPTOR),           // size of output buffer
				&bytesReturned,        // number of bytes returned
				0       // OVERLAPPED structure
			);

			//Dealloc
			CloseHandle(driveHandle);

			return dtd.TrimEnabled;
		}
		else {
			throw KV2Exception("Unable to obtain drive handle.");
			std::cin.ignore(LLONG_MAX);
			return false;
		}
	}

public:

	// Shred a file
	static void ShredFile(const path& inputPath)
	{
		if (DataOnSSD(inputPath)) {
			Colors::TextColor(YELLOW, BLACK);
			printf("\n\nFile at \"%s\" is on an SSD, and thus cannot be shredded.\nIf you would like to securely erase this file, \
it is recommended that you either wipe the drive or execute a trim.", inputPath.string().c_str());
			Colors::TextColor(WHITE, BLACK);
		}
		else if (!WriteRandomData(inputPath))
		{
			Colors::TextColor(YELLOW, BLACK);
			printf("\n\nFILE AT \"%s\" FAILED TO SHRED.", inputPath.string().c_str());
			Colors::TextColor(WHITE, BLACK);
			return;
		}
		remove(inputPath);
	}

};