#include <iostream>
#include <sstream>
#include <fstream>
#include <string>
#include <vector>
#include <cstdio>
#include <cstdlib>
#include <filesystem>

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

public:

	// Shred a file
	static void ShredFile(const path& inputPath)
	{
		if (!WriteRandomData(inputPath))
		{
			printf("\n\nFILE AT \"%s\" FAILED TO SHRED. USER IS ADVISED TO RESOLVE.", inputPath.string().c_str());
			return;
		}

		remove(inputPath);
	}

};