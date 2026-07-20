'use server';
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
export async function getSession(){
    const cookieStore=await cookies();
    const token=cookieStore.get("session")?.value;
    if (!token){
        return null;
    }
   
    const res=await fetch(`${process.env.BACKEND_URL}/api/me`,{
        headers:{Authorization:`Bearer ${token}`},
        cache: "no-store",
    });
    if (!res.ok){
        return null;
    }
    return res.json();
}
export async function redirectIfAuthenticated(destination="/"){
    const session=await getSession();
    if (session){
        redirect(destination);
    }
}
export async function requireSession(destination="/login"){
    const session=await getSession();
    if (!session){
        redirect(destination);
    }
    return session;
}
export async function getToken(){
    const cookieStore=await cookies();
    const token=cookieStore.get("session")?.value;
    if (!token){
        return null;
    }
    return token;
}