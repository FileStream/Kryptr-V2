#pragma once
#ifndef __KV2_EXCEPTIONS__
#define __KV2_EXCEPTIONS__

class KV2Exception {
private:
	const char* msg;
public:
	KV2Exception(const char* ms) : msg(ms) {}

	const char* what() {
		return this->msg;
	}
};

#endif