'use server';
import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import RegisterForm from "./RegisterForm";

export async function handleRegister(prevState:any,formData:FormData){
        const session = await getSession();
        if (session){
            redirect("/");
        }
        const username=formData.get("username") as string;
        const email=formData.get("email") as string;
        const password=formData.get("password") as string;
        const passwordVerify=formData.get("password-verify") as string;
        if (password.length<8){
            return {success:false, message:"The password should be at least 8 characters long"}
        }
        if (password!=passwordVerify){
            return {success:false, message:"The passwords are not the same!"}
        }

        const emailRegex=/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if(!emailRegex.test(email)){
            return {success:false, message:"The email is invalid"}
        }
        
        const response=await fetch(`${process.env.BACKEND_URL}/api/register`,{ //so its not hardcoded when it moves to prod
            method:"POST",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify({username,email,password})
        });
        if (!response.ok){
            return {success:false, message:"Something went wrong"}
        }
        await redirect("/");
        return {success:true, message:`Succesfully created account ${username}!`}
        
    }