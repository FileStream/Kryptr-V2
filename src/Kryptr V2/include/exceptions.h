#pragma once
#ifndef __KV2_EXCEPTIONS__
#define __KV2_EXCEPTIONS__

class KV2Exception {
private:
	const char* msg;
public:
	KV2Exception(const char* ms) : msg(ms) {}
	KV2Exception(std::string ms) : msg(ms.c_str()) {}

	const char* what() {
		return this->msg;
	}
};

#endif