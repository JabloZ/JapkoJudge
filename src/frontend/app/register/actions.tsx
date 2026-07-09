'use server';

export async function handleRegister(prevState:any,formData:FormData){
        const username=formData.get("username") as string;
        const email=formData.get("email") as string;
        const password=formData.get("password") as string;
        const passwordVerify=formData.get("password-verify") as string;
        if (password.length<8){
            return {error:"The password should be at least 8 characters long"}
        }
        if (password!=passwordVerify){
            return {error:"The passwords are not the same!"}
        }

        const emailRegex=/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if(!emailRegex.test(email)){
            return {error:"The email is invalid"}
        }
        
        const response=await fetch(`${process.env.BACKEND_URL}/api/register`,{
            method:"POST",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify({username,email,password})
        });
        if (!response.ok){
            return {error:"Something went wrong"}
        }
        return {success:true}
    }