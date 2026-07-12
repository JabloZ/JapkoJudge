import Link from "next/link";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { logout } from "./login/actions";
export default async function Home(){
  //TODO - MIDDLEWARE THAT REDIRECTS BEFORE WEBSITE STARTS RENDERING
   const session = await getSession();

    if (!session) {
        redirect("/login");
    }

    return <div>Witaj, {session.username}!
    <div>
      <Link href={"/login"}>Login</Link>
      <Link href={"/register"}>Register</Link> 
      <form action={logout}>
        <button type="submit">Logout</button>
      </form>
    </div>
    </div>;

}