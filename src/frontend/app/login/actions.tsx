'use server';

import { use } from "react";
import {cookies} from "next/headers";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";

export async function logout() {
    const cookieStore = await cookies();
    cookieStore.delete("session");
    redirect("/login")
}
export async function handleLogin(prevState:any, formData:FormData){
    const session = await getSession();
        if (session) {
            redirect("/");
        }
    const username=formData.get("username") as string;
    const password=formData.get("password") as string;
    const response=await fetch(`${process.env.BACKEND_URL}/api/login`,{
        method:"POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify({username,password})
    });
    
    const data=await response.json();

    if (!response.ok){
        return {success:false,message:data.message}
    }
    const cookieStore=await cookies();
    cookieStore.set("session",data.token,{
        httpOnly:true,
        secure: process.env.NODE_ENV==="production",
        sameSite:"lax",
        path:"/",
        maxAge:60*60*24*7});

    //TODO - LOGIN SESSION
    //data.token - moj jwt token
    return {success:true,message: data.message}
}//