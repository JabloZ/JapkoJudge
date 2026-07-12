'use server';
import { cookies } from "next/headers";

export async function getSession(){
    const cookieStore=await cookies();
    const token=cookieStore.get("session")?.value;
    if (!token){
        return null;
    }
    console.log(token);
    const res=await fetch(`${process.env.BACKEND_URL}/api/me`,{
        headers:{Authorization:`Bearer ${token}`},
        cache: "no-store",
    });
    if (!res.ok){
        return null;
    }
    return res.json();
}