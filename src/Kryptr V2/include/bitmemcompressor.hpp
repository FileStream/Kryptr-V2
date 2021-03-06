/*
 * bit7z - A C++ static library to interface with the 7-zip DLLs.
 * Copyright (c) 2014-2018  Riccardo Ostani - All Rights Reserved.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * Bit7z is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with bit7z; if not, see https://www.gnu.org/licenses/.
 */

#ifndef BITMEMCOMPRESSOR_HPP
#define BITMEMCOMPRESSOR_HPP

#include <iostream>
#include <vector>

#include "../include/bit7zlibrary.hpp"
#include "../include/bitformat.hpp"
#include "../include/bittypes.hpp"
#include "../include/bitarchivecreator.hpp"

namespace bit7z {
    using std::wstring;
    using std::vector;

    /**
     * @brief The BitMemCompressor class allows to compress memory buffers to the filesystem or to other memory buffers.
     *
     * It let decide various properties of the produced archive file, such as the password
     * protection and the compression level desired.
     */
    class BitMemCompressor : public BitArchiveCreator {
        public:
            /**
             * @brief Constructs a BitMemCompressor object.
             *
             * The Bit7zLibrary parameter is needed in order to have access to the functionalities
             * of the 7z DLLs. On the other hand, the BitInOutFormat is required in order to know the
             * format of the output archive.
             *
             * @param lib       the 7z library used.
             * @param format    the output archive format.
             */
            BitMemCompressor( Bit7zLibrary const& lib, BitInOutFormat const& format );

            /**
             * @brief Compresses the given buffer to an archive on the filesystem.
             *
             * @param in_buffer         the buffer to be compressed.
             * @param out_archive       the output archive path.
             * @param in_buffer_name    (optional) the buffer name used to give a name to the content of the archive.
             */
            void compress( const vector< byte_t >& in_buffer, const wstring& out_archive,
                           wstring in_buffer_name = L"" ) const;

            /**
             * @brief Compresses the given input buffer to the output buffer.
             *
             * @note If the format of the output doesn't support in memory compression, a BitException is thrown.
             *
             * @param in_buffer         the buffer to be compressed.
             * @param out_buffer        the buffer going to contain the output archive.
             * @param in_buffer_name    (optional) the buffer name used to give a name to the content of the archive.
             */
            void compress( const vector< byte_t >& in_buffer, vector< byte_t >& out_buffer,
                           const wstring& in_buffer_name = L"" ) const;
    };
}
#endif // BITMEMCOMPRESSOR_HPP
